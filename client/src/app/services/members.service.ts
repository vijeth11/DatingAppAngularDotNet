import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of, take, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParams } from '../_models/UserParams';
import { AccountsService } from './accounts.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelpers';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
  memberCache = new Map();
  user: User | undefined;
  userParams: UserParams | undefined;

  constructor(private http: HttpClient, private accountService: AccountsService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        this.userParams = new UserParams(user!);
        this.user = user!;
      }
    })
  }

  getUserParams(): UserParams {
    return this.userParams!;
  }

  setUserParams(userParams: UserParams) {
    this.userParams = userParams;
  }

  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }
  getMembers(userParams: UserParams) {
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    if (response) return of(response);

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(
         map(response => {
            this.memberCache.set(Object.values(userParams).join('-'), response);
            return response;
         })
      );
  }

  getMember(username: string) {
    const member = [...new Set<Member>([...this.memberCache.values()]
      .reduce((acc: Member[], y: PaginatedResult<Member[]>) => acc.concat(y.result || []), []))]
      .find((x: Member) => x.userName === username);
    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/name/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member)
      /*.pipe(
      tap(_ => {
        const index = this.members.indexOf(member);
        this.members[index] = { ...this.members[index], ...member };
      })
    )*/;
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'Users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return getPaginatedResult<Member[]>(this.baseUrl + 'likes', params, this.http);
  }

  
}
