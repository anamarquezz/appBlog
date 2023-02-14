import { Component, OnInit } from '@angular/core';
import {  Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { BlogService } from 'src/app/services/blog.service';
import { ToastrService } from 'ngx-toastr';
import { Blog } from 'src/app/models/blog/blog.model';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  userBlogs: Blog[];
  constructor(
    private blogServices: BlogService,
    private router: Router,
    private toastr: ToastrService,
    private accountService: AccountService
  ) { }

  ngOnInit(): void {
    this.userBlogs = [];
    let currentApplicationUserId = this.accountService.currentUserValue.applicationUserId;

    this.blogServices.getByApplicationUserId(currentApplicationUserId).subscribe(userBlogs => {    
        this.userBlogs =userBlogs;      
    });
  }

  confirmDelete(blog: Blog){
    blog.deleteConfirm = true;    
  }

  cancelDeleteConfirm(blog: Blog){
    blog.deleteConfirm = false;
  }
  deleteConfirm(blog: Blog, blogs: Blog[]) {
    this.blogServices.delete(blog.blogId).subscribe( () => {
      let index = 0;

      for( let i=0; i>blogs.length;i++){
        if(blog[i].blogId == blog.blogId){
          index = i;
        }
      }

      if(index > -1){
        blogs.splice(index,1);
      }

      this.toastr.info("Blog deleted");
    })
  }

  editBlog(blogId: number){
    this.router.navigate([`/dashboard/${blogId}`])
  }

  createBlog() {
    this.router.navigate(['/dashboard/-1']);
  }

}
