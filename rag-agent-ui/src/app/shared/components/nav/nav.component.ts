import { Component, inject } from '@angular/core';
import { ThemeService } from '../../../core/services/theme.service';
import { TranslationService, LanguageCode } from '../../../core/services/translation.service';

import { SHARED_IMPORTS } from '../../shared-imports';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss'],
  standalone: true,
  imports: [SHARED_IMPORTS]
})
export class NavComponent {
  readonly themeService = inject(ThemeService);
  readonly translateService = inject(TranslationService);

  readonly languages: { code: LanguageCode; label: string }[] = [
    { code: 'en', label: 'English' },
    { code: 'uk', label: 'Українська' },
    { code: 'pl', label: 'Polski' }
  ];

  setLanguage(lang: LanguageCode): void {
    this.translateService.setLanguage(lang);
  }
}
