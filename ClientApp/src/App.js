import React, { Component } from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { AddTest } from './components/AddTest'
import { ManageTests } from './components/ManageTests'
import { EditTest } from './components/EditTest'
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
        <Layout>
            <Route exact path='/' component={Home} />
            <Route exact path='/fetch-data' component={FetchData} />
            <AuthorizeRoute path='/add-test' component={AddTest} />
            <AuthorizeRoute exact path="/manage-tests" component={ManageTests} />
            <AuthorizeRoute path="/edit-test" component={EditTest} />
            <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
      </Layout>
    );
  }
}