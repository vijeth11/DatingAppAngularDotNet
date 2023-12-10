import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MessageService } from '../../services/message.service';
import { Message } from '../../_models/message';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?:NgForm;
  @Input() username?: string;
  messageContent = ''
  loading = false;

  constructor(public messageService: MessageService) { }

  ngOnInit() {

  }

  sendMessage() {
    if (!this.username) return;
    this.loading = true;
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm?.reset();
    }).finally(() => this.loading = false)
  }
}
