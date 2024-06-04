import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, CanActivate } from "@angular/router";
import { AccountService } from "../../account/account.service";
import { SharedService } from "../shared.service";
import { Observable } from "rxjs";
import { User } from "../models/Account/user";
import { map } from "rxjs/operators";
import { ToastrService } from "ngx-toastr";

@Injectable({
  providedIn: 'root'
})
export class AuthorizationGuard implements CanActivate {
  constructor(
    private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router,
    public toster:ToastrService
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.accountService.user$.pipe(
      map((user: User | null) => {
        if (user) {
          return true;
        } else {
          this.toster.error('Authorized users can view players')
         // alert("Access denied");
         // this.sharedService.showNotification(false,'Access Decliend')
         this.router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
          return false;
        }
      })
    );
  }
}
