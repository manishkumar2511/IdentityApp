import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { response } from 'express';
import { error } from 'console';
import { Router } from '@angular/router';
import { User } from '../../shared/models/Account/user';
import { take } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']  
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  submitted = false;
  errorMessages: string[] = [];
  isLoading = false;

  constructor(
    private accountService: AccountService,
    private formBuilder: FormBuilder,
    private router:Router,
    public toster:ToastrService
  ) {
    this.registerForm = new FormGroup({});
    this.accountService.user$.pipe(take(1)).subscribe({
      next:(user:User|null)=>{
        if(user){
          this.router.navigateByUrl('/');
        }
      }
    })
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15)]],
      lastName: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15)]],
      email: ['', [Validators.required, Validators.pattern('^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$')]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(15)]]
    });
  }

  register() {
    this.submitted = true;
    this.errorMessages=[];
    this.isLoading = true;
   // if (this.registerForm.valid) {
      this.accountService.register(this.registerForm.value).subscribe({
        next: (response:any) => {
          console.log(response.value.message);
          this.toster.success(response.value.message);
          this.router.navigateByUrl('/account/login');
          this.isLoading = false;

        },
        error: (error) => {
          console.log(error);
          this.toster.error(error.error);

          if(error.error.errors){
            this.errorMessages=error.error.errors;
            this.toster.error(error);
          }
          else{
            this.errorMessages.push(error.error)
          }
          this.isLoading = false;
        }
      });
    }
  }
