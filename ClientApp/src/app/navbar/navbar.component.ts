import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account/account.service';
import { User } from '../shared/models/Account/user';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  user: User | null = null;  // Store user data locally

  constructor(public accountService: AccountService) {}

  ngOnInit(): void {
    this.loadUser();
  }

  loadUser() {
    // Subscribing to the AccountService to get the current user
    this.accountService.user$.subscribe(user => {
      this.user = user;  // The user will be null if not logged in
    });
  }

  logOut() {
    this.accountService.logOut();
  }
  AuthLogin(){
   
    this.accountService.AuthLogin();
  }
}
