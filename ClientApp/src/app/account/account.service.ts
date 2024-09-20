import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../shared/models/Account/register';
import { environment } from '../../environments/environment.development';
import { Login } from '../shared/models/Account/login';
import { User } from '../shared/models/Account/user';
import { ReplaySubject, map, of } from 'rxjs';
import { response } from 'express';
import { Router } from '@angular/router';
import { ConfirmEmail } from '../shared/models/Account/confirmEmail';
import { ResetPassword } from '../shared/models/Account/resetPassword';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private userSource=new ReplaySubject<User|null>(1);
  user$=this.userSource.asObservable();

  constructor(
    private http:HttpClient,
    private router:Router
  ) { }
  register(model:Register){
    return this.http.post('https://localhost:44385/api/Account/register',model);
  }

  confirmEmail(model:ConfirmEmail){
    return this.http.put('https://localhost:44385/api/Account/confirm-email',model);
  }
  resendEmailConfirmationLink(email: string) {
  const url = `https://localhost:44385/api/Account/resend-email-confirmation-link/${email}`;
  return this.http.post(url, null);
}

forgetUsernameOrPassword(email: string) {
const url = `https://localhost:44385/api/Account/forget-username-or-password/${email}`;
return this.http.post(url, null);
}
resetPassword(model:ResetPassword){
  return this.http.put('https://localhost:44385/api/Account/reset-password',model);
}



  login(model: Login) {
    return this.http.post<User>('https://localhost:44385/api/Account/login', model)
      .pipe(
        map((user: User) => {
          if (user) {
            this.setUser(user);
          }
        })
      );
  }
  getJWT() {
    const key = localStorage.getItem(environment.userKey);
    if (key) {
      const user: User = JSON.parse(key);
      return user.jwt;
    } else {
      return null;
    }
  }

  setUser(user:User){
    localStorage.setItem(environment.userKey,JSON.stringify(user));
    this.userSource.next(user);
  }
  refreshUSer(jwt:string|null){
    if (jwt === null) {
      this.userSource.next(null);
      return of(undefined);
    }
    let headers = new HttpHeaders();
    headers = headers.set('Authorization', `Bearer ${jwt}`);
    return this.http.get<User>('https://localhost:44385/api/Account/refresh-user-token', { headers })
      .pipe(
        map((user: User) => {
          if (user) {
            this.setUser(user);
          }
        })
      );
  }
  logOut(){
    localStorage.removeItem(environment.userKey);
    this.userSource.next(null);
    this.router.navigateByUrl('/');
  }
 
  // Update this method to perform a redirect to your SSO login page
// Redirect to your SSO MVC controller login action
AuthLogin() {
  const returnUrl = encodeURIComponent(window.location.href); // The current page URL
  window.location.href = `https://localhost:7047/Account/Login?returnUrl=${returnUrl}`;
}


}
