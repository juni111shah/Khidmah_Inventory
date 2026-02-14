export interface Company {
  id: string;
  name: string;
  legalName?: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  logoUrl?: string;
  isActive: boolean;
  defaultWarehouseId?: string;
  theme?: string;
  createdAt?: string;
}

export interface CreateCompanyRequest {
  name: string;
  legalName?: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
}

export interface UpdateCompanyRequest extends CreateCompanyRequest {
  isActive?: boolean;
  defaultWarehouseId?: string;
}

export interface CompanyUser {
  userId: string;
  userName: string;
  email: string;
  isDefault: boolean;
  isActive: boolean;
}
