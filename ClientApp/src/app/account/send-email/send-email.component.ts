import { Component, OnInit } from '@angular/core';
import { SharedService } from '../../shared/shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../account.service';
import { take } from 'rxjs';
import { User } from '../../shared/models/Account/user';
import { response } from 'express';
import { ToastrService } from 'ngx-toastr';
import { error } from 'console';

@Component({
  selector: 'app-send-email',
  templateUrl: './send-email.component.html',
  styleUrl: './send-email.component.css'
})
export class SendEmailComponent implements OnInit {
  emailForm:FormGroup=new FormGroup({});
  submitted=false;
  mode:string|undefined;
  errorMessages:string[]=[];
  constructor(
    private accountService:AccountService,
    private sharedService:SharedService,
    private formBuilder:FormBuilder,
    private router:Router,
    private activatedRoute:ActivatedRoute,
    private toster:ToastrService
  ){}
  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
      next:(user:User|null)=>{
        if(user){
          this.router.navigateByUrl('/');
        }else{
          const mode=this.activatedRoute.snapshot.paramMap.get('mode');
          if(mode){
            this.mode=mode;
            console.log(mode);
            this.initializeForm();
          }
        }
      }
    });
  }
  initializeForm() {
    this.emailForm = this.formBuilder.group({
      email:['', Validators.required],
    });
  }
  sendEmail(){
    this.submitted=true;
    this.errorMessages=[];
    if(this.emailForm.valid && this.mode){
      debugger
      if(this.mode.includes('resend-email-confirmation-link')){
        this.accountService.resendEmailConfirmationLink(this.emailForm.get('email')?.value).subscribe({
          next:(response:any)=>{
            this.toster.success(response.value.message);
            this.router.navigateByUrl('/account/login');
          },
          error: (error) => {
            console.log(error.error);
            this.toster.error(error.error);
            this.router.navigateByUrl('/account/login');

  
            if(error.error.errors){
              this.errorMessages=error.error.errors;
              this.toster.error(error.error.error);
            }
            else{
              this.errorMessages.push(error.error)
            }
          }
        })
      }
      else if(this.mode.includes('forget-username-or-password')){
        this.accountService.forgetUsernameOrPassword(this.emailForm.get('email')?.value).subscribe({
          next:(response:any)=>{
            console.log(response.message)
            console.log(response.value)
            this.toster.success(response.value.message);
            this.router.navigateByUrl('/account/login')
          },
          error: (error) => {
            console.log(error.error);
            this.toster.error(error.error);
  
            if(error.error.errors){
              this.errorMessages=error.error.errors;
              this.toster.error(error.error.error);
            }
            else{
              this.errorMessages.push(error.error)
            }
          }
        });
      }
    }
  }
  cancel(){
    this.router.navigateByUrl('/account/login');
  }

}
