import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../../services/auth/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-auth',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css'
})
export class AuthComponent {
  authForm: FormGroup;
  isLogin = true;

  constructor(
    private authService: AuthService,
    private fb: FormBuilder
  ) {
    this.authForm = this.fb.group({
      fullName: [''],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  toggleMode(event: Event) {
    event.preventDefault();
    this.isLogin = !this.isLogin;
    this.authForm.reset();
  }

  onSubmit() {
    if (this.authForm.invalid) return;

    if (this.isLogin) {
      this.authService.login(this.authForm.value.email, this.authForm.value.password).subscribe({
        next: res => {
          this.authService.storeToken(res.token);
          location.reload();
        },
        error: err => console.error(err)
      });
    } else {
      this.authService.register(this.authForm.value).subscribe({
        next: res => alert('Registered successfully!'),
        error: err => console.error(err)
      });
    }
  }

  closeModal() {
    // emit
  }
}
