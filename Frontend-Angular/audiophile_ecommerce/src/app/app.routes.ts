import { Routes } from '@angular/router';
import { CategoryComponent } from './pages/category/category.component';

export const routes: Routes = [
    {
        path: 'category/:type', component: CategoryComponent
    }
];
