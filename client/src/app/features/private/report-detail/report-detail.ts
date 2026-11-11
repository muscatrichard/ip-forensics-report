import { Component, computed, inject, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RouterLink } from '@angular/router';
import { ReportsService } from '../../../core/services/reports.service';
import {IpForensicsReport} from '../ip-forensics-report/ip-forensics-report';
import {toErrorMessage} from '../../../core/utils/error-message';

@Component({
  imports: [RouterLink, MatButtonModule, MatIconModule, MatProgressBarModule, IpForensicsReport],
  selector: 'app-report-detail',
  styleUrl: './report-detail.scss',
  templateUrl: './report-detail.html',
})
export class ReportDetail {
  readonly id = input.required<string>();
  protected readonly report = inject(ReportsService).reportResource(this.id);
  protected readonly errorMessage = computed(() => toErrorMessage(this.report.error()));

}
