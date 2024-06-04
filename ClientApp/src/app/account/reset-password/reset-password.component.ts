import { Component } from '@angular/core';
import { AccountService } from '../account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from '../../shared/shared.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { User } from '../../shared/models/Account/user';
import { ResetPassword } from '../../shared/models/Account/resetPassword';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent {
  resetPasswordForm:FormGroup=new FormGroup({});
  token:string|undefined;
  email:string|undefined;
  errorMessages:string[]=[]
  submitted=true;

  constructor(
    private accountService:AccountService,
    private sharedService:SharedService,
    private formBuilder:FormBuilder,
    private router:Router,
    private activatedRoute:ActivatedRoute,
    private toster:ToastrService,
  ){}

  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
      
      next:(user:User|null)=>{
        
        if(user){
          this.router.navigateByUrl('/');
        }
        else{
          this.activatedRoute.queryParamMap.subscribe({
            next:(params:any)=>{
             
              this.token=params.get('token'),
              this.email=params.get('email')

             
              if(this.token && this.email){
                this.initializeForm(this.email);
              }
              
            }
          })
        }
      }
    });
  }
  initializeForm(username:string) {
    this.resetPasswordForm = this.formBuilder.group({
      email:[{value:username, disbaled:true}],
      newPassword:['', [Validators.required, Validators.minLength(6), Validators.maxLength(15)]]
    });
  }
  resetPassword(){
    this.submitted=true;
    this.errorMessages=[];
    if(this.resetPasswordForm.valid && this.email && this.token){
      const model:ResetPassword={
        token:this.token,
        email:this.email,
        newPassword:this.resetPasswordForm.get('newPassword')?.value
      };
      this.accountService.resetPassword(model).subscribe({
        next:(response:any)=>{
          this.toster.success(response.value.message);
          this.router.navigateByUrl('/account/login');
        },
        error: (error) => {
          console.log(error);
          this.toster.error(error.error);
        }
      });
    }
  }
  cancel(){
    this.router.navigateByUrl('/account/login');
  }

}
