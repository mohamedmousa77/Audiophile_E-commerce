import { Component } from '@angular/core';

@Component({
  selector: 'app-about',
  imports: [],
  templateUrl: './about.component.html',
  styleUrl: './about.component.css'
})
export class AboutComponent {
  aboutTitle = "bringing you the best audio gear";
  aboutDescription = "Located at the heart of Egypt, Audiophile is the premier store for high end headphones, earphones, speakers, and demonstration rooms available for you to browse and ";
  aboutImageUrl = "about-image.png";
  aboutAlt = "About cover image";

}
