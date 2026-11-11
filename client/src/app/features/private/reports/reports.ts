import { DatePipe } from '@angular/common';
import { Component, computed, inject, linkedSignal, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTableModule } from '@angular/material/table';
import { Router, RouterLink } from '@angular/router';
import { ReportsService } from '../../../core/services/reports.service';
import {AbuseScoreLabel} from '../abuse-score-label/abuse-score-label';
import {PagedResult} from '../../../core/models/pagination.model';
import {ReportSummary} from '../../../core/models/report.model';
import {toErrorMessage} from '../../../core/utils/error-message';



@Component({
  imports: [
    DatePipe,
    RouterLink,
    MatButtonModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatTableModule,
    AbuseScoreLabel
  ],
  selector: 'app-reports',
  styleUrl: './reports.scss',
  templateUrl: './reports.html',
})
export class Reports {
  private readonly router = inject(Router);

  protected readonly columns = ['ipAddress', 'createdAtUtc', 'abuseConfidenceScore', 'country'];
  protected readonly pageSizeOptions = [10, 25, 50];


  protected readonly pageIndex = signal(0);
  protected readonly pageSize = signal(10);

  protected readonly reports = inject(ReportsService).reportsResource(() => ({
    page: this.pageIndex() + 1,
    pageSize: this.pageSize(),
  }));


  private readonly lastPage = linkedSignal<
    PagedResult<ReportSummary> | undefined,
    PagedResult<ReportSummary> | undefined
  >({
    source: () => (this.reports.hasValue() ? this.reports.value() : undefined),
    computation: (next, previous) => next ?? previous?.value,
  });

  protected readonly rows = computed(() => this.lastPage()?.items ?? []);
  protected readonly totalCount = computed(() => this.lastPage()?.totalCount ?? 0);
  protected readonly isEmpty = computed(() => this.reports.hasValue() && this.totalCount() === 0);
  protected readonly errorMessage = computed(() => {
    const error = this.reports.error();
    return error ? (toErrorMessage(error) ?? 'Could not load reports.') : null;
  });

  protected onPage(event: PageEvent): void {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
  }

  protected open(row: ReportSummary): void {
    void this.router.navigate(['/reports', row.id]);
  }
}
