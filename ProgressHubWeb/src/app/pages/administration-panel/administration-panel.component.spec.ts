import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministrationPanelComponent } from './administration-panel.component';

describe('AdministrationPanelComponent', () => {
  let component: AdministrationPanelComponent;
  let fixture: ComponentFixture<AdministrationPanelComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AdministrationPanelComponent]
    });
    fixture = TestBed.createComponent(AdministrationPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
