export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: AuthUser;
}

export interface AuthUser {
  id: string;
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  companyId: string;
  roles: string[];
  permissions: string[];
}

export interface RegisterRequest {
  email: string;
  userName: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  companyId: string;
}

export interface RegisterResponse {
  userId: string;
  email: string;
  userName: string;
}

export interface RefreshTokenRequest {
  token: string;
  refreshToken: string;
}

export interface RefreshTokenResponse {
  token: string;
  refreshToken: string;
  expiresAt: string;
}

