import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import MainLayout from './MainLayout.tsx'
import {createHashRouter, createRoutesFromElements, Route, RouterProvider} from "react-router-dom";
import Home from "@/pages/page.tsx";
import Settings from "@/pages/settings/page.tsx";
import '@/lib/i18n';

const router = createHashRouter(
    createRoutesFromElements(
        <Route path="/" element={<MainLayout />}>
            <Route index element={<Home />} />
            <Route path="settings" element={<Settings />} />
        </Route>
    )
);

const root = createRoot(document.getElementById('root')!)
root.render(
    <StrictMode>
        <RouterProvider router={router}/>
    </StrictMode>,
)
