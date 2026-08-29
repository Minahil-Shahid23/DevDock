export interface Project {
  id: string;
  name: string;
  description: string;
  ownerId: string;
  ownerName: string;
  createdAt: string;
  memberCount: number;
  taskCount: number;
}

export interface CreateProjectPayload {
  name: string;
  description: string;
}