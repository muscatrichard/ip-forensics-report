import { Component, DestroyRef, ElementRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { finalize } from 'rxjs';
import { Report } from '../../../core/models/report.model';
import { ReportsService } from '../../../core/services/reports.service';
import { focusFirstInvalidControl } from '../../../core/utils/form-utils';
import {IpForensicsReport} from '../ip-forensics-report/ip-forensics-report';
import {toErrorMessage} from '../../../core/utils/error-message';


@Component({
  imports: [ReactiveFormsModule, MatButtonModule, MatFormFieldModule, MatInputModule, IpForensicsReport],
  selector: 'app-report-generation',
  styleUrl: './report-generation.scss',
  templateUrl: './report-generation.html',
})
export class ReportGeneration {
  private readonly reports = inject(ReportsService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly host = inject<ElementRef<HTMLElement>>(ElementRef);

  protected readonly form = inject(NonNullableFormBuilder).group({
    ipAddress: ['', Validators.required],
  });

  protected readonly submitting = signal(false);
  protected readonly formError = signal<string | null>(null);
  protected readonly report = signal<Report | null>(null);

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      focusFirstInvalidControl(this.host.nativeElement);
      return;
    }

    this.submitting.set(true);
    this.formError.set(null);

    this.reports
      .generate({ ipAddress: this.form.controls.ipAddress.value.trim() })
      .pipe(
        finalize(() => this.submitting.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (report) => {
          this.report.set(report);
        },
        error: (error: unknown) => this.formError.set(toErrorMessage(error))
      });
  }
}
