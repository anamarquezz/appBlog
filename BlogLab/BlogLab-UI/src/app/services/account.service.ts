import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BehaviorSubject, Observable } from 'rxjs';
import { ApplicationUserCreate } from '../models/account/application-user-created.model';
import { ApplicationUserLogin } from '../models/account/application-user-login-model';
import { ApplicationUser } from '../models/account/applocation-user-model';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private currentUserSubject$: BehaviorSubject<ApplicationUser>

  constructor(
    private http: HttpClient
  ) { 

    this.currentUserSubject$ = new BehaviorSubject<ApplicationUser>(JSON.parse(localStorage.getItem('blogLab-currentUser')));

  }

  login(model: ApplicationUserLogin): Observable<ApplicationUser>{

    return this.http.post(`${environment.webapi}/Account/login`, model).pipe(
      map((user : ApplicationUser) =>{

        if(user){
          localStorage.setItem('blogLab-currentUser', JSON.stringify(user));
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  register(model: ApplicationUserCreate) : Observable<ApplicationUser> {

    return this.http.post(`${environment.webapi}/Account/register`, model).pipe(
      map((user : ApplicationUser) =>{

        if(user){
          localStorage.setItem('blogLab-currentUser', JSON.stringify(user));
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  setCurrentUser(user: ApplicationUser){
    this.currentUserSubject$.next(user);   
  }

  public get CurrentUserValue(): ApplicationUser {
    return this.currentUserSubject$.value;
  }

  public isLoggedIn() {
    const currentUser = this.CurrentUserValue;
    const isLoggedIn = currentUser && !!currentUser.token;
    return isLoggedIn;
  }

  logout(){
    localStorage.removeItem('blogLab-currentUser');
    this.currentUserSubject$.next(null);
  }
}
