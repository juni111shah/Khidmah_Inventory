import { Injectable, OnDestroy } from '@angular/core';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { filter, map } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { HeaderService } from './header.service';

export interface RouteHeaderData {
  title: string;
  description?: string;
}

@Injectable({
  providedIn: 'root'
})
export class RouteHeaderService implements OnDestroy {
  private subscription?: Subscription;

  constructor(
    private router: Router,
    private headerService: HeaderService
  ) {}

  initialize(): void {
    // Listen to route changes and update header
    this.subscription = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => {
        let route = this.router.routerState.root;
        // Get the deepest child route
        while (route.firstChild) {
          route = route.firstChild;
        }
        return route;
      }),
      map(route => route.snapshot.data)
    ).subscribe((data: any) => {
      if (data['header']) {
        const headerData: RouteHeaderData = data['header'];
        this.headerService.setHeaderInfo({
          title: headerData.title,
          description: headerData.description
        });
      } else if (data['title']) {
        // Support simple title format
        this.headerService.setHeaderInfo({
          title: data['title'],
          description: data['description']
        });
      }
    });

    // Set initial header on first load
    this.setHeaderFromCurrentRoute();
  }

  private setHeaderFromCurrentRoute(): void {
    let route = this.router.routerState.root;
    while (route.firstChild) {
      route = route.firstChild;
    }
    
    const data = route.snapshot.data;
    if (data['header']) {
      const headerData: RouteHeaderData = data['header'];
      this.headerService.setHeaderInfo({
        title: headerData.title,
        description: headerData.description
      });
    } else if (data['title']) {
      this.headerService.setHeaderInfo({
        title: data['title'],
        description: data['description']
      });
    }
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}

