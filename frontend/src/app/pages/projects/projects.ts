import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ProjectService } from '../../services/project';
import { Auth } from '../../services/auth';
import { Project } from '../../models/project.model';

@Component({
  selector: 'app-projects',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './projects.html',
  styleUrl: './projects.css'
})
export class Projects implements OnInit {
  private projectService = inject(ProjectService);
  private fb = inject(FormBuilder);
  authService = inject(Auth);

  projects = signal<Project[]>([]);
  isLoading = signal(true);
  showCreateForm = signal(false);

  createForm = this.fb.group({
    name: ['', Validators.required],
    description: ['']
  });

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.isLoading.set(true);
    this.projectService.getMyProjects().subscribe({
      next: (data) => {
        this.projects.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onCreateProject(): void {
    if (this.createForm.invalid) return;

    this.projectService
      .createProject(this.createForm.getRawValue() as { name: string; description: string })
      .subscribe({
        next: () => {
          this.createForm.reset();
          this.showCreateForm.set(false);
          this.loadProjects();
        }
      });
  }

  logout(): void {
    this.authService.logout();
  }
}