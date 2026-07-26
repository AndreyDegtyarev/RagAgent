import { Component, inject } from '@angular/core';
import { TranslationService } from '../../core/services/translation.service';

import { SHARED_IMPORTS } from '../../shared/shared-imports';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  standalone: true,
  imports: [SHARED_IMPORTS]
})
export class HomeComponent {
  readonly translateService = inject(TranslationService);
}
