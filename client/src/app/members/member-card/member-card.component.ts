import { Component, Input } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from '../../services/members.service';
import { PresenceService } from '../../services/presence.service';
import { Member } from '../../_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {

  @Input() member: Member | undefined;

  constructor(private memberService: MembersService,
    private toastrService: ToastrService,
    public presenceService: PresenceService) { }

  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe({
      next: () => this.toastrService.success("You have liked " + member.knownAs)
    });
  }

}
