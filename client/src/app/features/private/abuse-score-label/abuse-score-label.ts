import {Component, computed, input} from '@angular/core';

interface AbuseBand {
  key: 'low' | 'caution' | 'elevated' | 'critical';
  label: string;
}

@Component({
  imports: [],
  selector: 'app-abuse-score-label',
  styleUrl: './abuse-score-label.scss',
  templateUrl: './abuse-score-label.html',
})
export class AbuseScoreLabel {
  readonly score = input.required<number>();

  protected readonly band = computed(() => this.abuseBand(this.score()));

  private abuseBand(score: number): AbuseBand {
    if (score >= 75) return { key: 'critical', label: 'Critical' };
    if (score >= 20) return { key: 'elevated', label: 'Elevated' };
    if (score >= 1) return { key: 'caution', label: 'Caution' };
    return { key: 'low', label: 'Low Risk' };
  }
}
