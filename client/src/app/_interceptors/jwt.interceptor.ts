import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountsService } from '../services/accounts.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService:AccountsService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.accountService.currentUser$.pipe(take(1)).subscribe(
      {
        next: user => {
          if (user) {
            request = request.clone({
              setHeaders: {
                Authorization: 'Bearer ' + user.token
              }
            });
          }
        }
    })
    return next.handle(request);
  }
}
