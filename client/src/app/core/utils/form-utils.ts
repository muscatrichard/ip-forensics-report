export function focusFirstInvalidControl(root: HTMLElement): void {
  root.querySelector<HTMLElement>('[formControlName].ng-invalid')?.focus();
}
