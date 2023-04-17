import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountsService } from '../services/accounts.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister: EventEmitter<boolean> = new EventEmitter <boolean>();

  model: any = {}


  constructor(private accountService: AccountsService, private toastrService: ToastrService) { }

  ngOnInit(): void {
  }

  register() {
    this.accountService.register(this.model).subscribe(
      {
        next: response => {
          console.log(response);
          this.cancel();
        },
        error: error => this.toastrService.error(error.error)
      }
    );
  }

  cancel() {
    console.log("cancelled");
    this.cancelRegister.emit(false);
  }
}
