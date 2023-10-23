import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs';
import { AccountsService } from '../../services/accounts.service';
import { MembersService } from '../../services/members.service';
import { MessageService } from '../../services/message.service';
import { PresenceService } from '../../services/presence.service';
import { Member } from '../../_models/member';
import { Message } from '../../_models/message';
import { User } from '../../_models/user';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, AfterViewInit, OnDestroy {
  member: Member = {} as Member;
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  @ViewChild('memberTabs') memberTabs?: TabsetComponent;
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;

  public get lastActiveDate(): number {
    return (new Date(this.member?.lastActive||'')).valueOf();
  }

  constructor(private accountService: AccountsService,
    private route: ActivatedRoute,
    private messageService: MessageService,
    public presenceService: PresenceService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => { if (user) this.user = user; }
    })
  }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    })
   
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview:false
      }
    ];

    this.galleryImages = this.getImages();
  }

  ngAfterViewInit() {
    this.route.queryParams.subscribe(
      {
        next: (params) => {
          params['tab'] && this.selectTab(params['tab']);
        }
      });
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading == 'Messages' && this.user) {
      this.messageService.createhubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }

  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName)
        .subscribe({
          next: messages => this.messages = messages
        });
    }
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }

  private getImages(): NgxGalleryImage[] {
    if (!this.member) return [];
    const imageUrls = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url
      });
    }
    return imageUrls;
  }

  ngOnDestroy() {
    this.messageService.stopHubConnection();
  }
}
