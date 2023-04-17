import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;

  constructor(private http: HttpClient) {
  }

  ngOnInit(): void {
    this.getUsers();
  }

  getUsers() {
    this.http.get('http://localhost:5130/api/Users').subscribe(data => {
      this.users = data;
      console.log(this.users);
    });
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(value: boolean) {
    this.registerMode = value;
  }

}
