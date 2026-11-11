import { HttpClient, httpResource } from '@angular/common/http';
import { Service, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {GenerateReportRequest, ReportSummary} from '../models/report.model';
import {PagedResult} from '../models/pagination.model';
import {Report} from '../models/report.model';


@Service()
export class ReportsService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/reports';


  reportsResource(params: () => { page: number; pageSize: number }) {
    return httpResource<PagedResult<ReportSummary>>(() => ({
      url: this.baseUrl,
      params: params(),
    }));
  }


  reportResource(id: () => string) {
    return httpResource<Report>(() => `${this.baseUrl}/${id()}`);
  }

  generate(request: GenerateReportRequest): Observable<Report> {
    return this.http.post<Report>(this.baseUrl, request);
  }
}
