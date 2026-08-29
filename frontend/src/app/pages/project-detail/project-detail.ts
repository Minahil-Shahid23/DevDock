import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ProjectService } from '../../services/project';
import { TaskService } from '../../services/task';
import { Project } from '../../models/project.model';
import { Task, TaskStatus } from '../../models/task.model';

@Component({
  selector: 'app-project-detail',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './project-detail.html',
  styleUrl: './project-detail.css'
})
export class ProjectDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private projectService = inject(ProjectService);
  private taskService = inject(TaskService);
  private fb = inject(FormBuilder);

  projectId = '';
  project = signal<Project | null>(null);
  tasks = signal<Task[]>([]);
  isLoading = signal(true);
  showCreateForm = signal(false);
todoTasks = computed(() => this.tasks().filter((t) => t.status === 0));
inProgressTasks = computed(() => this.tasks().filter((t) => t.status === 1));
inReviewTasks = computed(() => this.tasks().filter((t) => t.status === 2));
doneTasks = computed(() => this.tasks().filter((t) => t.status === 3));
  createForm = this.fb.group({
    title: ['', Validators.required],
    description: [''],
    priority: [1, Validators.required] // default Medium
  });

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('id') || '';
    this.loadProject();
    this.loadTasks();
  }

  loadProject(): void {
    this.projectService.getProjectById(this.projectId).subscribe({
      next: (data) => this.project.set(data)
    });
  }

  loadTasks(): void {
    this.isLoading.set(true);
    this.taskService.getProjectTasks(this.projectId).subscribe({
      next: (data) => {
        this.tasks.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
onCreateTask(): void {
  if (this.createForm.invalid) return;

  const raw = this.createForm.getRawValue();
  this.taskService
    .createTask(this.projectId, {
      title: raw.title!,
      description: raw.description || '',
      priority: Number(raw.priority)   // 👈 explicitly Number mein convert kiya
    })
    .subscribe({
      next: () => {
        this.createForm.reset({ priority: 1 });
        this.showCreateForm.set(false);
        this.loadTasks();
      },
      error: (err) => {
        console.error('Task creation failed:', err);
      }
    });
}

  moveTask(task: Task, newStatus: number): void {
    this.taskService.updateTask(task.id, { status: newStatus }).subscribe({
      next: () => this.loadTasks()
    });
  }

  deleteTask(taskId: string): void {
    this.taskService.deleteTask(taskId).subscribe({
      next: () => this.loadTasks()
    });
  }
}