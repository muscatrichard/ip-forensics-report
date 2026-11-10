import { HttpErrorResponse } from '@angular/common/http';

export function toErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) return 'Cannot reach the server. Check your connection.';
    if (error.status >= 500) return 'Server error. Try again later.';

    const detail = readDetail(error.error);
    if (detail) return detail;
  }
  return 'Something went wrong.';
}

function readDetail(body: unknown): string | null {
  return typeof body === 'object' && body !== null && 'detail' in body && typeof body.detail === 'string'
    ? body.detail
    : null;
}
