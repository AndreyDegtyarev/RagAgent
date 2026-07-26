import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { finalize, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface Document {
  id: string;
  originalFileName: string;
  storagePath: string;
  contentType: string;
  fileSize: number;
  fileHash: string | null;
  status: number; // 1 = Created, 2 = FileStored, 3 = Processing, 4 = Completed, 5 = Failed
  createdAt: string;
  completedAt: string | null;
}

export interface UploadResponse {
  documentId: string;
  status: string;
}

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/documents`;

  // Signals for state management
  readonly documents = signal<Document[]>([]);
  readonly isLoading = signal<boolean>(false);
  readonly isUploading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  fetchDocuments(): void {
    this.isLoading.set(true);
    this.error.set(null);
    this.http.get<Document[]>(this.apiUrl)
      .pipe(
        finalize(() => this.isLoading.set(false))
      )
      .subscribe({
        next: (docs) => this.documents.set(docs),
        error: (err) => {
          this.error.set(err.message || 'Failed to fetch documents');
          console.error('Error fetching documents:', err);
        }
      });
  }

  uploadDocument(file: File) {
    this.isUploading.set(true);
    this.error.set(null);
    
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post<UploadResponse>(this.apiUrl, formData)
      .pipe(
        tap(() => this.fetchDocuments()), // Refresh list on success
        finalize(() => this.isUploading.set(false))
      );
  }
}
