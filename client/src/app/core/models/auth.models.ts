export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiresAtUtc: string;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
}