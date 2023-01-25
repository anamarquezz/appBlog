import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../services/account.service';
import { catchError } from 'rxjs/operators';


@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(
    private toaster: ToastrService,
    private accountService: AccountService    
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if(error){
          switch(error.status){
            case 400:
              this.handle400Error(error);
              break;
            case 401:
              this.handle401Error(error);
              break;
            case 500:
              this.handle500Error(error);
              break;
            default:
              this.handleUnexpedtedError(error);
              break;
          }
        }
        return throwError(error);
      })
    );
  }

  handle400Error(error: any){
    if (!!error.error && Array.isArray(error.error)){ //if that is an array
      let errorMessage ='';
      for (const key in error.error){
        if(!!error.error[key]){
        const errorElement = error.error[key];
        errorMessage = (`${errorMessage}${errorElement.code} - ${errorElement.description}\n`)
      }
      }
      this.toaster.error(errorMessage, error.statusText);
      console.log(error.error);
    }else if(!!error?.error.errors?.content && (typeof error.error.errors.Content) === 'object'){
      let errorObject = error.error.errors.Content;
      let errorMessage = '';
      for (const key in errorObject) {
        const errorElement = errorObject[key];
        errorMessage = (`${errorMessage}${errorElement}\n`)
      }
      this.toaster.error(errorMessage, error.statusCode);
      console.log(error.error);
    }else if(!!error.error){
      let errorMessage = ((typeof error.error) == 'string') ? error.error : 'There was a validation error';
      this.toaster.error(errorMessage, error.statusCode);
      console.log(error.error);
    }else{
      this.toaster.error(error.statusText, error.status);
      console.log(error);
    }    
  }
  handle401Error(error: any){
    let errorMessage = 'please login to your account.';
    this.accountService.logout();
    this.toaster.error(errorMessage, error.statusText);
    //route to the login page
  }
  handle500Error(error: any){
    let errorMessage = "Please constact the Administrator. An error happend in the server.";
    this.accountService.logout()
    console.log(error);
  }
  handleUnexpedtedError(error: any){
    this.toaster.error('Something unexpecte happend.')
    console.log(error);
  }
}
