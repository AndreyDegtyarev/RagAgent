import { Injectable, effect, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  readonly isDarkMode = signal<boolean>(this.getInitialTheme());

  constructor() {
    // Reactively update the DOM class based on theme state signal
    effect(() => {
      const dark = this.isDarkMode();
      if (dark) {
        document.documentElement.classList.add('dark-theme');
      } else {
        document.documentElement.classList.remove('dark-theme');
      }
      localStorage.setItem('rag-theme-dark', String(dark));
    });
  }

  toggleTheme(): void {
    this.isDarkMode.update(dark => !dark);
  }

  private getInitialTheme(): boolean {
    const saved = localStorage.getItem('rag-theme-dark');
    if (saved !== null) {
      return saved === 'true';
    }
    // Default to dark theme if user prefers dark mode
    return window.matchMedia('(prefers-color-scheme: dark)').matches;
  }
}
