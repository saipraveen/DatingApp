import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: import('@angular/common/http').HttpRequest<any>,
    next: import('@angular/common/http').HttpHandler
  ): import('rxjs').Observable<import('@angular/common/http').HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(err => {
        // 401 error - Unauthorized.
        if (err.status === 401) {
          return throwError(err.statusText);
        }

        // Application Errors - the errors thrown by API (500 internal server errors.)
        if (err instanceof HttpErrorResponse) {
          const applicationError = err.headers.get('Application-Error');
          if (applicationError) {
            return throwError(applicationError);
          }
        }

        // Model State Errors
        const serverError = err.error;
        let modalStateErrors = '';
        if (serverError.errors && typeof serverError.errors === 'object') {
          for (const key in serverError.errors) {
            if (serverError.errors[key]) {
              modalStateErrors += serverError.errors[key] + '\n';
            }
          }
        }
        return throwError(modalStateErrors || serverError || 'Server Error');
      })
    );
  }
}

export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true
};
