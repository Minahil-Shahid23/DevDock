export type TaskStatus = 'Todo' | 'InProgress' | 'InReview' | 'Done';
export type TaskPriority = 'Low' | 'Medium' | 'High' | 'Urgent';

export interface Task {
  id: string;
  title: string;
  description: string;
  projectId: string;
  assignedToId: string | null;
  assignedToName: string | null;
  createdById: string;
  createdByName: string;
  status: number;      // 0=Todo, 1=InProgress, 2=InReview, 3=Done
  priority: number;     // 0=Low, 1=Medium, 2=High, 3=Urgent
  deadline: string | null;
  createdAt: string;
}

export interface CreateTaskPayload {
  title: string;
  description: string;
  assignedToId?: string;
  priority: number; // 0=Low, 1=Medium, 2=High, 3=Urgent
  deadline?: string;
}

export interface UpdateTaskPayload {
  status?: number; // 0=Todo, 1=InProgress, 2=InReview, 3=Done
  title?: string;
  description?: string;
}