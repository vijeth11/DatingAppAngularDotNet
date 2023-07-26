import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountsService } from '../services/accounts.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Output() cancelRegister: EventEmitter<boolean> = new EventEmitter <boolean>();

  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;

  constructor(private accountService: AccountsService,
    private toastrService: ToastrService,
    private route: Router,
    private fb: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required,Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues("password")]]
    });
    // validate confirmPassword and password matches when password input is changed by triggering confirmpassword input
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    }); 
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control.value == control.parent?.get(matchTo)?.value ? null : { notMatching: true };
    }
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const value = { ...this.registerForm.value, dateOfBirth: dob };
    this.accountService.register(value).subscribe(
      {
        next: response => {
          this.route.navigateByUrl('/members');
        },
        error: error => {
          this.validationErrors = error;
        }
      }
    );
  }

  cancel() {
    console.log("cancelled");
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined): string | undefined{
    if (!dob) return;
    const newdob = new Date(dob);
    return new Date(newdob.setMinutes(newdob.getMinutes() - newdob.getTimezoneOffset())).toISOString().slice(0,10);
  }
}
