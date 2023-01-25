import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Photo } from '../models/photo/photo.model';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {

  constructor(
    private http: HttpClient
  ) { }

  create(model: FormData): Observable<Photo>{
    return this.http.post<Photo>(`${environment.webapi}/Photo`, model);
    
  }
  getByApplicationuserId(): Observable<Photo[]>{
    return this.http.get<Photo[]>(`${environment.webapi}/Photo`);

  }

  get(photoId: number): Observable<Photo>{
    return this.http.get<Photo>(`${environment.webapi}/Photo/${photoId}`);
  }
  
  delete(photoId: number): Observable<number>{
    return this.http.delete<number>(`${environment.webapi}/Photo/${photoId}`);
  }
}
