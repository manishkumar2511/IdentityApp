import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { take, switchMap } from "rxjs/operators";
import { AccountService } from "../../account/account.service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private accountService: AccountService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.accountService.user$.pipe(
      take(1),
      switchMap(user => {
        if (user) {
          const clonedRequest = req.clone({
            setHeaders: {
              Authorization: `Bearer ${user.jwt}`
            }
          });
          return next.handle(clonedRequest);
        } else {
          return next.handle(req);
        }
      })
    );
  }
}
