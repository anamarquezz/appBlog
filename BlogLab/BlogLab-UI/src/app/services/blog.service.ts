import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { BlogComment } from '../models/blog-comment/blog-comment.model';
import { BlogCreate } from '../models/blog/blog-create.model';
import { BlogPaging } from '../models/blog/blog-paging.model';
import { Blog } from '../models/blog/blog.model';
import { PageResult } from '../models/blog/paged-result.model';

@Injectable({
  providedIn: 'root'
})
export class BlogService {

  constructor(
    private http: HttpClient
  ) { }

  create(model: BlogCreate): Observable<Blog>{
    return this.http.post<Blog>(`${environment.webapi}/Blog`, model);
  }

  getAll(blogPaging : BlogPaging) : Observable<PageResult<Blog>>{
    return this.http.get<PageResult<Blog>>(
      `${environment.webapi}/Blog?Page=${blogPaging.page}&PageSize=${blogPaging.pageSize})`); 
  }
  get(blogId: number): Observable<Blog> {
    return this.http.get<Blog>(`${environment.webapi}/Blog/${blogId})`); 
  }
  getByApplicationUserId(applicationUserId: number) : Observable<Blog[]>{
    return this.http.get<Blog[]>(`${environment.webapi}/Blog/user/${applicationUserId})`); 
  }
  getMostFamous(){
    return this.http.get<Blog[]>(`${environment.webapi}/Blog/famous`); 
  }
  delete(blogId: number){
    return this.http.get<number>(`${environment.webapi}/Blog/${blogId}`);
  }
}
