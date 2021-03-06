import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/User';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberListResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 5;

  constructor(
    private userService: UserService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  // FIXMEOS -- here the return type should be Observable<PaginatedResult<User[]>>, check how this is working.
  resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
    // FIXMEOS -- check why this tslint waring is shown.
    // tslint:disable-next-line: no-string-literal
    return this.userService.getUsers(this.pageNumber, this.pageSize).pipe(
      catchError(error => {
        this.alertify.error('Problem retrieving data.');
        this.router.navigate(['/']);
        return of(null);
      })
    );
  }
}
