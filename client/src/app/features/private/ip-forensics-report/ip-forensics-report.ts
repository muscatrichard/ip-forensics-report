import { DatePipe } from '@angular/common';
import { Component, computed, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Report } from '../../../core/models/report.model';
import {AbuseScoreLabel} from '../abuse-score-label/abuse-score-label';

@Component({
  imports: [DatePipe, MatCardModule, MatIconModule, AbuseScoreLabel],
  selector: 'app-ip-forensics-report',
  styleUrl: './ip-forensics-report.scss',
  templateUrl: './ip-forensics-report.html',
})
export class IpForensicsReport {
  readonly report = input.required<Report>();

  protected readonly networkAttributes = computed(() => {
    const { mobile, proxy, hosting, tor } = this.report().network;
    return [
      { label: 'Mobile connection', value: mobile },
      { label: 'Proxied connection', value: proxy },
      { label: 'Hosted service', value: hosting },
      { label: 'Tor network', value: tor },
    ];
  });
}
