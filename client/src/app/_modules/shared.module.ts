import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';



@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ToastrModule.forRoot(),
    BsDropdownModule.forRoot()
  ],
  exports: [
    CommonModule,
    ToastrModule,
    BsDropdownModule
  ]
})
export class SharedModule { }
