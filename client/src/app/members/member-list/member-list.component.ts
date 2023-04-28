import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { MembersService } from '../../services/members.service';
import { Member } from '../../_models/member';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  members$: Observable<Member[]> | undefined;

  constructor(private memberService: MembersService) {
  }

  ngOnInit(): void {
    this.members$ = this.memberService.getMembers();
  }

}
