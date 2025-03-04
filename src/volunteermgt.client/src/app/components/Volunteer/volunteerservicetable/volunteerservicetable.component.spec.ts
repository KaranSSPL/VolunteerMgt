import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VolunteerservicetableComponent } from './volunteerservicetable.component';

describe('VolunteerservicetableComponent', () => {
  let component: VolunteerservicetableComponent;
  let fixture: ComponentFixture<VolunteerservicetableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [VolunteerservicetableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VolunteerservicetableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
