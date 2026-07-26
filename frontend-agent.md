# RagAgent Angular Frontend Agent Specification (Standalone)

This document defines the architecture, project structure, design rules, and coding standards for building the **RagAgent** frontend application using Angular standalone components.

Any Frontend Agent or developer working on the user interface must strictly follow the specifications outlined below.

---

## 1. Architectural Overview

The frontend is a single-page application (SPA) built using the following stack:
* **Framework**: Angular 19 (Latest)
* **UI Component Library**: Angular Material 19 (MDC-based components)
* **Component Architecture**: **Standalone Components**. No NgModules are permitted. Every component, directive, and pipe must declare its own dependencies via the `imports` metadata.
* **Common Imports Optimization**: A centralized `SHARED_IMPORTS` array must be created and exported from a shared file to prevent verbose replication of material and common imports across multiple components.
* **Reactivity & State Management**: **Angular Signals** (`signal`, `computed`, `effect`, signal-based inputs, model inputs, and queries) for local and global state.
* **HTTP Client**: Angular's `HttpClient` for API requests, configured in app config and accessed via `inject()`.

---

## 2. Project Initialisation & Setup

To ensure the workspace matches the standalone and Material architecture, the project must be initialized as follows:

### A. Prerequisites
Ensure the latest stable Node.js (v20+ or v22+ LTS) is installed, along with Angular CLI v19:
```bash
npm install -g @angular/cli@19
```

### B. Project Generation
Since Angular 19 defaults to generating standalone applications, you can simply run:
```bash
ng new rag-agent-ui --standalone --routing --style=scss
cd rag-agent-ui
```

### C. Add Angular Material
Integrate Angular Material for components and styling:
```bash
ng add @angular/material
```
*Choose a prebuilt theme (e.g., Indigo/Pink or Purple/Green) or setup a Custom Theme, and enable global Angular Material typography styles and browser animations.*

---

## 3. Directory & Standalone Structure

The project follows a modular folder structure without using `NgModule` classes. Code must be organized cleanly:

```
rag-agent-ui/
├── src/
│   ├── app/
│   │   ├── core/                  # Core Services (Guards, interceptors, services)
│   │   │   ├── services/          # API services
│   │   │   │   └── document.service.ts
│   │   │   └── interceptors/      # Http interceptors
│   │   ├── shared/                # Shared Utilities and Imports
│   │   │   └── shared-imports.ts  # Exposes the SHARED_IMPORTS array
│   │   ├── features/              # Feature Standalone Components
│   │   │   ├── home/              # Home landing page component
│   │   │   │   ├── home.component.ts | html | scss
│   │   │   └── job/               # RAG Documents dashboard / Job component
│   │   │       ├── job.component.ts | html | scss
│   │   ├── app.routes.ts          # Routes configuration (replaces app-routing.module)
│   │   ├── app.config.ts          # Application providers configuration
│   │   ├── app.component.ts       # Root Standalone Component
│   │   ├── app.component.html
│   │   └── app.component.scss
│   ├── main.ts                    # Entry point bootstrapping the application
```

---

## 4. Common Imports Design (`shared-imports.ts`)

To avoid declaring long lists of imports (such as forms, common utilities, and Angular Material design modules) in every component, create a unified imports array:

```typescript
// src/app/shared/shared-imports.ts
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Material Modules
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

export const SHARED_IMPORTS = [
  CommonModule,
  RouterLink,
  RouterLinkActive,
  FormsModule,
  ReactiveFormsModule,
  
  MatToolbarModule,
  MatButtonModule,
  MatIconModule,
  MatMenuModule,
  MatTableModule,
  MatCardModule,
  MatProgressBarModule,
  MatProgressSpinnerModule,
  MatFormFieldModule,
  MatInputModule,
  MatSnackBarModule,
  MatTooltipModule
] as const;
```

---

## 5. Standalone Component Guidelines

Every component must be declared with `standalone: true` and specify its dependencies in its `imports` array.

### Example Component Definition
```typescript
import { Component } from '@angular/core';
import { SHARED_IMPORTS } from '../../shared/shared-imports';

@Component({
  selector: 'app-document-upload',
  templateUrl: './document-upload.component.html',
  styleUrls: ['./document-upload.component.scss'],
  standalone: true,
  imports: [SHARED_IMPORTS] // Single import grants access to all shared utilities and material components
})
export class DocumentUploadComponent {
  // Component logic
}
```

---

## 6. Application Bootstrapping (`main.ts`)

Bootstrap the application directly in `src/main.ts` using `bootstrapApplication` with the proper routing and animation configuration:

```typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';

import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    provideAnimations()
  ]
}).catch(err => console.error(err));
```

---

## 7. Angular Signals Guidelines

Use Signals for component local states, derived values, inputs, outputs, and queries:

### A. Signal State
Use `signal` and `computed` for reactively binding local data:
```typescript
readonly searchQuery = signal<string>('');
readonly documents = signal<any[]>([]);

readonly filteredDocuments = computed(() => {
  const query = this.searchQuery().toLowerCase().trim();
  return this.documents().filter(doc => doc.name.toLowerCase().includes(query));
});
```

### B. Signal Inputs and Outputs
Use modern signal-based properties:
```typescript
readonly doc = input.required<{ id: string; name: string }>();
readonly isExpanded = model<boolean>(false); // Two-way binding
readonly delete = output<string>(); // Custom events
```

### C. Signal Queries
Query templates reactively:
```typescript
readonly fileInput = viewChild.required<ElementRef<HTMLInputElement>>('fileInput');
```

---

## 8. Quality & Review Checklist

Before finalizing standalone tasks, verify implementation against the checklist below:

* [ ] **Standalone Declaration**: Verify every component, directive, and pipe is decorated with `standalone: true`.
* [ ] **No AppModule**: Ensure there are no legacy `NgModule` class files (like `AppModule`, `SharedModule`, `MaterialModule`).
* [ ] **Shared Imports Usage**: Verify components import the shared `SHARED_IMPORTS` bundle array for common items instead of duplicating the import block.
* [ ] **Providers Registration**: Verify core providers (router, http, animations) are registered inside `main.ts` or `app.config.ts`.
* [ ] **Signal Inputs & Queries**: Confirm `@Input()`, `@Output()`, and `@ViewChild()` have been migrated to their signal-based counterparts.
