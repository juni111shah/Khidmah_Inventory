export interface Role {
  id: string;
  name: string;
  description?: string;
  isSystemRole: boolean;
  userCount: number;
  permissionCount: number;
  permissions: Permission[];
  createdAt: string;
  updatedAt: string;
}

export interface Permission {
  id: string;
  name: string;
  description?: string;
  module: string;
  action: string;
}

export interface CreateRoleRequest {
  name: string;
  description?: string;
  permissionIds: string[];
}

export interface UpdateRoleRequest {
  name: string;
  description?: string;
  permissionIds: string[];
}

export interface AssignRoleToUserRequest {
  userId: string;
  roleId: string;
}

