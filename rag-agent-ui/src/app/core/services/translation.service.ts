import { Injectable, computed, signal } from '@angular/core';

export type LanguageCode = 'en' | 'uk' | 'pl';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  readonly currentLanguage = signal<LanguageCode>(this.getInitialLanguage());

  private readonly translationsData: Record<LanguageCode, Record<string, string>> = {
    en: {
      'NAV_HOME': 'Home',
      'NAV_DOCUMENTS': 'Documents',
      'WELCOME_TITLE': 'Welcome to RagAgent',
      'WELCOME_SUBTITLE': 'Your intelligent Retrieval-Augmented Generation (RAG) assistant',
      'WELCOME_DESC': 'RagAgent is a state-of-the-art document processing and search interface. Upload your documents, track their vector indexing jobs in real time, and ask context-aware questions from your data.',
      'UPLOAD_TITLE': 'Document Upload',
      'UPLOAD_DRAG_DROP': 'Drag and drop your PDF or TXT files here, or click to browse',
      'UPLOAD_SUPPORTED': 'Supported formats: PDF, TXT (Max 10MB)',
      'UPLOAD_STATUS_SUCCESS': 'Document uploaded successfully and is now processing.',
      'UPLOAD_STATUS_FAILED': 'Failed to upload document. Please try again.',
      'DOCS_TITLE': 'Document Processing Jobs',
      'DOCS_FILTER_PLACEHOLDER': 'Search documents by name...',
      'COL_ID': 'ID',
      'COL_NAME': 'File Name',
      'COL_SIZE': 'Size',
      'COL_STATUS': 'Status',
      'COL_CREATED': 'Created At',
      'STATUS_CREATED': 'Created',
      'STATUS_STORED': 'File Stored',
      'STATUS_PROCESSING': 'Processing',
      'STATUS_COMPLETED': 'Completed',
      'STATUS_FAILED': 'Failed',
      'REFRESH': 'Refresh',
      'UPLOAD_BUTTON': 'Upload Document',
      'FOOTER_TEXT': 'Powered by .NET 10 & Angular 19 with pgvector & Ollama',
      'NO_DOCUMENTS': 'No documents found. Upload your first document to get started!'
    },
    uk: {
      'NAV_HOME': 'Головна',
      'NAV_DOCUMENTS': 'Документи',
      'WELCOME_TITLE': 'Ласкаво просимо до RagAgent',
      'WELCOME_SUBTITLE': 'Ваш інтелектуальний помічник з пошуково-доповненої генерації (RAG)',
      'WELCOME_DESC': 'RagAgent — це сучасна система обробки документів та пошуку. Завантажуйте свої документи, відстежуйте процес векторної індексації в реальному часі та ставте питання на основі ваших даних.',
      'UPLOAD_TITLE': 'Завантаження документа',
      'UPLOAD_DRAG_DROP': 'Перетягніть файли PDF або TXT сюди або натисніть, щоб вибрати',
      'UPLOAD_SUPPORTED': 'Підтримувані формати: PDF, TXT (макс. 10 МБ)',
      'UPLOAD_STATUS_SUCCESS': 'Документ успішно завантажено, триває обробка.',
      'UPLOAD_STATUS_FAILED': 'Не вдалося завантажити документ. Спробуйте ще раз.',
      'DOCS_TITLE': 'Завдання з обробки документів',
      'DOCS_FILTER_PLACEHOLDER': 'Пошук документів за назвою...',
      'COL_ID': 'ID',
      'COL_NAME': 'Назва файлу',
      'COL_SIZE': 'Розмір',
      'COL_STATUS': 'Статус',
      'COL_CREATED': 'Створено',
      'STATUS_CREATED': 'Створено',
      'STATUS_STORED': 'Файл збережено',
      'STATUS_PROCESSING': 'Обробка',
      'STATUS_COMPLETED': 'Завершено',
      'STATUS_FAILED': 'Помилка',
      'REFRESH': 'Оновити',
      'UPLOAD_BUTTON': 'Завантажити документ',
      'FOOTER_TEXT': 'Працює на .NET 10 & Angular 19 з pgvector & Ollama',
      'NO_DOCUMENTS': 'Документів не знайдено. Завантажте свій перший документ для початку!'
    },
    pl: {
      'NAV_HOME': 'Główna',
      'NAV_DOCUMENTS': 'Dokumenty',
      'WELCOME_TITLE': 'Witamy w RagAgent',
      'WELCOME_SUBTITLE': 'Twój inteligentny asystent RAG (Retrieval-Augmented Generation)',
      'WELCOME_DESC': 'RagAgent to zaawansowany system przetwarzania i wyszukiwania dokumentów. Prześlij swoje dokumenty, śledź ich indeksowanie wektorowe w czasie rzeczywistym i zadawaj pytania na podstawie swoich danych.',
      'UPLOAD_TITLE': 'Prześlij dokument',
      'UPLOAD_DRAG_DROP': 'Przeciągnij i upuść pliki PDF lub TXT tutaj lub kliknij, aby przeglądać',
      'UPLOAD_SUPPORTED': 'Obsługiwane formaty: PDF, TXT (maks. 10 MB)',
      'UPLOAD_STATUS_SUCCESS': 'Dokument został pomyślnie przesłany i jest przetwarzany.',
      'UPLOAD_STATUS_FAILED': 'Nie udało się przesłać dokumentu. Spróbuj ponownie.',
      'DOCS_TITLE': 'Zadania przetwarzania dokumentów',
      'DOCS_FILTER_PLACEHOLDER': 'Wyszukaj dokumenty po nazwie...',
      'COL_ID': 'ID',
      'COL_NAME': 'Nazwa pliku',
      'COL_SIZE': 'Rozmiar',
      'COL_STATUS': 'Status',
      'COL_CREATED': 'Utworzono',
      'STATUS_CREATED': 'Utworzony',
      'STATUS_STORED': 'Plik zapisany',
      'STATUS_PROCESSING': 'Przetwarzanie',
      'STATUS_COMPLETED': 'Zakończono',
      'STATUS_FAILED': 'Błąd',
      'REFRESH': 'Odśwież',
      'UPLOAD_BUTTON': 'Prześlij dokument',
      'FOOTER_TEXT': 'Oparte na .NET 10 & Angular 19 z pgvector & Ollama',
      'NO_DOCUMENTS': 'Nie znaleziono dokumentów. Prześlij swój pierwszy dokument, aby rozpocząć!'
    }
  };

  // Active translation dictionary derived reactively from currentLanguage signal
  readonly t = computed(() => this.translationsData[this.currentLanguage()]);

  // Method to easily translate a key
  translate(key: string): string {
    return this.t()[key] || key;
  }

  setLanguage(lang: LanguageCode): void {
    this.currentLanguage.set(lang);
    localStorage.setItem('rag-lang', lang);
  }

  private getInitialLanguage(): LanguageCode {
    const saved = localStorage.getItem('rag-lang') as LanguageCode;
    if (saved === 'en' || saved === 'uk' || saved === 'pl') {
      return saved;
    }
    // Check browser language defaults
    const browserLang = navigator.language.substring(0, 2);
    if (browserLang === 'uk' || browserLang === 'pl') {
      return browserLang as LanguageCode;
    }
    return 'en';
  }
}
