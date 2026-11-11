export interface ReportSummary {
  id: string;
  createdAtUtc: string;
  ipAddress: string;
  abuseConfidenceScore: number;
  country: string;
}


export interface Report {
  id: string;
  createdAtUtc: string;
  ipAddress: string;
  reputation: {
    abuseConfidenceScore: number;
    totalReports: number;
    lastReportedAt: string | null;
  };
  geolocation: {
    continent: string;
    country: string;
    region: string;
    city: string;
  };
  network: {
    mobile: boolean;
    proxy: boolean;
    hosting: boolean;
    tor: boolean;
  };
}

export interface GenerateReportRequest {
  ipAddress: string;
}
