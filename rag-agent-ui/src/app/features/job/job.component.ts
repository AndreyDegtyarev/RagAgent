import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DocumentService } from '../../core/services/document.service';
import { TranslationService } from '../../core/services/translation.service';
import { MatSnackBar } from '@angular/material/snack-bar';

import { SHARED_IMPORTS } from '../../shared/shared-imports';

@Component({
  selector: 'app-job',
  templateUrl: './job.component.html',
  styleUrls: ['./job.component.scss'],
  standalone: true,
  imports: [SHARED_IMPORTS]
})
export class JobComponent implements OnInit {
  readonly docService = inject(DocumentService);
  readonly translateService = inject(TranslationService);
  private readonly snackBar = inject(MatSnackBar);

  readonly searchQuery = signal<string>('');
  readonly selectedFile = signal<File | null>(null);

  // Computed signal to filter documents reactively
  readonly filteredDocuments = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    const docs = this.docService.documents();
    if (!query) return docs;
    return docs.filter(doc => doc.originalFileName.toLowerCase().includes(query));
  });

  // Material Table columns
  readonly displayedColumns: string[] = ['name', 'size', 'status', 'created'];

  ngOnInit(): void {
    this.refreshList();
  }

  refreshList(): void {
    this.docService.fetchDocuments();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile.set(input.files[0]);
    }
  }

  uploadSelectedFile(): void {
    const file = this.selectedFile();
    if (!file) return;

    this.docService.uploadDocument(file).subscribe({
      next: (res) => {
        this.selectedFile.set(null);
        this.snackBar.open(this.translateService.translate('UPLOAD_STATUS_SUCCESS'), 'OK', {
          duration: 4000,
          horizontalPosition: 'end',
          verticalPosition: 'bottom'
        });
      },
      error: () => {
        this.snackBar.open(this.translateService.translate('UPLOAD_STATUS_FAILED'), 'Dismiss', {
          duration: 4000,
          horizontalPosition: 'end',
          verticalPosition: 'bottom'
        });
      }
    });
  }

  formatBytes(bytes: number, decimals = 2): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const dm = decimals < 0 ? 0 : decimals;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }

  getStatusLabel(statusCode: number): string {
    switch (statusCode) {
      case 1: return this.translateService.translate('STATUS_CREATED');
      case 2: return this.translateService.translate('STATUS_STORED');
      case 3: return this.translateService.translate('STATUS_PROCESSING');
      case 4: return this.translateService.translate('STATUS_COMPLETED');
      case 5: return this.translateService.translate('STATUS_FAILED');
      default: return 'Unknown';
    }
  }

  getStatusClass(statusCode: number): string {
    switch (statusCode) {
      case 1: return 'status-created';
      case 2: return 'status-stored';
      case 3: return 'status-processing';
      case 4: return 'status-completed';
      case 5: return 'status-failed';
      default: return '';
    }
  }
}
