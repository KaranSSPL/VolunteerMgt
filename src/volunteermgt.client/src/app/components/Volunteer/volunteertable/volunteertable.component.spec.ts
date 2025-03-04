import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VolunteertableComponent } from './volunteertable.component';

describe('VolunteertableComponent', () => {
  let component: VolunteertableComponent;
  let fixture: ComponentFixture<VolunteertableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [VolunteertableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VolunteertableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
