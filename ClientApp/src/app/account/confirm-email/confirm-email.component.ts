import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { SharedService } from '../../shared/shared.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from '../../shared/models/Account/user';
import { Token } from '@angular/compiler';
import { ConfirmEmail } from '../../shared/models/Account/confirmEmail';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.css'
})
export class ConfirmEmailComponent implements OnInit {
  constructor(
    private accountService:AccountService,
    private sharedService:SharedService,
    private router:Router,
    private activatedRoute:ActivatedRoute,
    public toster:ToastrService
  ){}
  success=true;
  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
      next:(user:User|null)=>{
        if(user){
          this.router.navigateByUrl('/');
        }
        else{
          this.activatedRoute.queryParamMap.subscribe({
            next:(params:any)=>{
             const confirmEmail:ConfirmEmail={
              token:params.get('token'),
              email:params.get('email')

             }
             this.accountService.confirmEmail(confirmEmail).subscribe({
              next:(response:any)=>{
                this.toster.success(response.value.message);
              },
              error: (error) => {
                this.success=false;
                console.log(error);
                this.toster.error(error.error);
              }
             });
            }
          })
        }
      }
    });
  }
  resendEmailConfirmationLink(){
    this.router.navigateByUrl('/account/send-email/resend-email-confirmation-link')
  }

}
